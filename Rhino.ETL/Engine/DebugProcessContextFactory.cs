namespace Rhino.ETL.Engine
{
	using System;
	using System.Collections.Generic;
	using System.Diagnostics;
	using System.Text;
	using Retlang;

	public class DebugProcessContextFactory : IProcessContextFactory
	{
		private readonly IProcessContextFactory inner = new ProcessContextFactory();
		private readonly Dictionary<DebugProcessContext, string> liveProcesses = new Dictionary<DebugProcessContext, string>();

		public IProcessContext CreateAndStart()
		{
			IProcessContext context = inner.CreateAndStart();
			return new DebugProcessContext(context, this);
		}

		public IProcessContext Create()
		{
			IProcessContext context = inner.Create();
			return new DebugProcessContext(context, this);
		}

		public void Start()
		{
			inner.Start();
		}

		public void Stop()
		{
			if(liveProcesses.Count!=0)
			{
				StringBuilder sb = new StringBuilder();
				sb.AppendLine("Cannot close process factory when it has open processes: ");
				foreach (string value in liveProcesses.Values)
				{
					sb.AppendLine(value);
					sb.AppendLine("----");

				}
				throw new InvalidOperationException(sb.ToString());
			}
			inner.Stop();
		}

		public void Join()
		{
			inner.Join();
		}

		public class DebugProcessContext : IProcessContext
		{
			private readonly IProcessContext inner;
			private readonly DebugProcessContextFactory parent;

			public DebugProcessContext(IProcessContext inner, DebugProcessContextFactory parent)
			{
				this.inner = inner;
				this.parent = parent;
				lock(parent.liveProcesses)
				{
					StackTrace trace = new StackTrace();
					parent.liveProcesses.Add(this, trace.ToString());
				}
			}

			public void Start()
			{
				inner.Start();
			}

			public void Stop()
			{
				inner.Stop();
				parent.liveProcesses.Remove(this);
			}

			public void Join()
			{
				inner.Join();
			}

			public void Publish(ITransferEnvelope toPublish)
			{
				inner.Publish(toPublish);
			}

			public IUnsubscriber SubscribeToKeyedBatch<K, V>(ITopicMatcher topic, ResolveKey<K, V> keyResolver, On<IDictionary<K, IMessageEnvelope<V>>> target, int minBatchIntervalInMs)
			{
				return inner.SubscribeToKeyedBatch(topic, keyResolver, target, minBatchIntervalInMs);
			}

			public IUnsubscriber SubscribeToBatch<T>(ITopicMatcher topic, On<IList<IMessageEnvelope<T>>> msg, int minBatchIntervalInMs)
			{
				return inner.SubscribeToBatch(topic, msg, minBatchIntervalInMs);
			}

			public IUnsubscriber Subscribe<T>(ITopicMatcher topic, OnMessage<T> msg)
			{
				return inner.Subscribe(topic, msg);
			}

			public IRequestReply<T> SendRequest<T>(ITransferEnvelope env)
			{
				return inner.SendRequest<T>(env);
			}

			public IRequestReply<T> SendRequest<T>(object topic, object msg)
			{
				return inner.SendRequest<T>(topic, msg);
			}

			public object CreateUniqueTopic()
			{
				return inner.CreateUniqueTopic();
			}

			public event OnQueueFull QueueFullEvent
			{
				add { inner.QueueFullEvent += value; }
				remove { inner.QueueFullEvent -= value; }
			}

			public void Publish(object topic, object msg, object replyToTopic)
			{
				inner.Publish(topic, msg, replyToTopic);
			}

			public void Publish(object topic, object msg)
			{
				inner.Publish(topic, msg);
			}

			public void Enqueue(Command command)
			{
				inner.Enqueue(command);
			}

			public void Schedule(Command command, int firstIntervalInMs)
			{
				inner.Schedule(command, firstIntervalInMs);
			}

			public void ScheduleOnInterval(Command command, int firstIntervalInMs, int regularIntervalInMs)
			{
				inner.ScheduleOnInterval(command, firstIntervalInMs, regularIntervalInMs);
			}
		}
	}
}