namespace Rhino.ETL.Engine
{
	using System;
	using System.Collections.Generic;
	using System.ComponentModel;
	using Boo.Lang;
	using Exceptions;
	using Interfaces;
	using Retlang;
	using Rhino.ETL2.Impl;

	public abstract class Join : TransformationBase<Join>, IProcess
	{
		private ICallable condition;
		private bool leftDone = false;
		private bool rightDone = false;
		readonly List<Row> left = new List<Row>();
		readonly List<Row> right = new List<Row>();
		private bool hasOuterJoin = true;

		public bool HasOuterJoin
		{
			get { return hasOuterJoin; }
			set { hasOuterJoin = value; }
		}

		protected Join(string name)
		{
			this.name = name;
			EtlConfigurationContext.Current.AddJoin(name, this);
		}

		[Browsable(false)]
		public ICallable Condition
		{
			get { return condition; }
			set { condition = value; }
		}

		public override string Name
		{
			get { return name; }
		}

		public void Start(IProcessContext context, params string[] inputNames)
		{
			if (inputNames.Length != 2)
				throw new ArgumentException("join must have two input names");

			ColoredConsole.WriteLine(ConsoleColor.Magenta, "Starting join: " + Name);

			string leftName = inputNames[0];
			string rightName = inputNames[1];

			EtlConfigurationContext configurationContext = EtlConfigurationContext.Current;
			Pipeline currentPipeline = Pipeline.Current;

			Items[ProcessContextKey] = context;
			context.Subscribe<object>(new TopicEquals(leftName + Messages.Done), delegate
			{
				leftDone = true;
				using (configurationContext.EnterContext())
				using (currentPipeline.EnterContext())
					TryToComplete(context);
			});
			context.Subscribe<object>(new TopicEquals(rightName + Messages.Done), delegate
			{
				rightDone = true;
				using (configurationContext.EnterContext())
				using (currentPipeline.EnterContext())
					TryToComplete(context);
			});
			context.Subscribe<Row>(new TopicEquals(leftName), delegate(IMessageHeader header, Row msg)
			{
				left.Add(msg);
			});
			context.Subscribe<Row>(new TopicEquals(rightName), delegate(IMessageHeader header, Row msg)
			{
				right.Add(msg);
			});
		}

		private void TryToComplete(IProcessContext context)
		{
			if (leftDone == false || rightDone == false)
				return;

			try
			{
				JoinQueues(left, right);
			}
			catch (Exception ex)
			{
				string message = "Failed to process join "+ Name;
				Logger.Error(message, ex);
				context.Publish(Messages.Exception, new ScriptEvalException(message, ex));
				context.Stop();
				return;
			}
			string topic = Name + "." + OutputName + Messages.Done;
			ColoredConsole.WriteLine(ConsoleColor.DarkMagenta, string.Format("Finishing transform {0} {1} out rows, left {2} right {3}", topic, rowCount, left.Count, right.Count));
			context.Publish(topic, Messages.Done);
			context.Stop();
			left.Clear();
			right.Clear();
		}

		private void JoinQueues(IEnumerable<Row> leftRows, IEnumerable<Row> rightRows)
		{
			Dictionary<Row, bool> matched = new Dictionary<Row, bool>();
			foreach (Row leftRow in leftRows)
			{
				foreach (Row rightRow in rightRows)
				{
					bool shouldAdd = (bool)Condition.Call(new object[] { leftRow, rightRow });
					if (shouldAdd)
					{
						matched[leftRow] = true;
						matched[rightRow] = true;
						Apply(leftRow, rightRow);
					}
				}
			}
			if (HasOuterJoin)
			{
				foreach (Row row in leftRows)
				{
					if(matched.ContainsKey(row))
						continue;
					Row emptyRow = new Row();
					bool shouldAdd = (bool)Condition.Call(new object[] { row, emptyRow });
					if (shouldAdd)
						Apply(row, emptyRow);
				}
				foreach (Row row in rightRows)
				{
					if(matched.ContainsKey(row))
						continue;
					Row emptyRow = new Row();
					bool shouldAdd = (bool)Condition.Call(new object[] { emptyRow, row });
					if (shouldAdd)
						Apply(row, emptyRow);
				}
			}

		}

		private void Apply(Row leftValue, Row rightRow)
		{
			PrepareCurrentTransformParameters(new Row());
			DoApply(CurrentTransformParameters.Row, leftValue, rightRow);
			ForwardRow();
		}

		protected abstract void DoApply(Row Row, Row Left, Row Right);
	}
}