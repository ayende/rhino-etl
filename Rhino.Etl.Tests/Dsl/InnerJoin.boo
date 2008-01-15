
join get_user_roles:
	left:
		input "test", Command = "SELECT id, name, email  FROM Users"
		
	right:
		join:
			left:
				input "test", Command = "SELECT id, name FROM Roles"
			right:
				input "test", Command = "SELECT userid, roleid FROM User2Role"
			
			on Equals(left.id, right.roleid):
				row.Name = left.Name
				row.UserId = right.UserId
				
	on Equals(left.id, right.UserId):
		row.userid = left.id
		row.name = left.name
		row.role = right.name

process merge_user_roles:
	get_user_roles()
	
	aggregate:
		groupBy userid
		
		accumulate:
			aggregate.userid = row.userid
			aggregate.name = row.name
			aggregate.rolesList = [] if aggregate.rolesList is null
			aggregate.rolesList.Add(row.role)
		
		terminate:
			aggregate.roles = aggregate.name + " is: " +string.Join(", ", aggregate.rolesList.ToArray(string))
		
			
	output "test", Command = """
		UPDATE Users
		SET roles = @roles
		WHERE Id = @userid
		"""