
innerJoin get_user_roles_hash:
    left id:
        input "test", Command = "SELECT id, name, email  FROM Users"

    right UserId:
        innerJoin:
            left id:
                input "test", Command = "SELECT id, name FROM Roles"
            right roleid:
                input "test", Command = "SELECT userid, roleid FROM User2Role"
            action:
                row.Name = left.Name
                row.UserId = right.UserId    
                
    action:
        row.userid = left.id
        row.name = left.name
        row.role = right.name

process merge_user_roles_hash:
    get_user_roles_hash()
    
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