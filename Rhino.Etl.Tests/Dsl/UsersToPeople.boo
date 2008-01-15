operation split_name:
	for row in rows:
		continue if row.Name is null
		row.FirstName = row.Name.Split()[0]
		row.LastName = row.Name.Split()[1]
		yield row
	
process UsersToPeople:
	input "test", Command = "SELECT id, name, email  FROM Users"
	split_name()
	output "test", Command = """
		INSERT INTO People (UserId, FirstName, LastName, Email) 
		VALUES (@UserId, @FirstName, @LastName, @Email)
		""":
		row.UserId = row.Id
		