operation split_name_bulk:
	for row in rows:
		continue if row.Name is null
		row.FirstName = row.Name.Split()[0]
		row.LastName = row.Name.Split()[1]
		yield row
	
process UsersToPeopleBulk:
	input "test", Command = "SELECT id, name, email  FROM Users"
	split_name_bulk()
	sqlBulkInsert "test", "People", TableLock = true :
		map "id", int
		map "firstname"
		map "lastname"
		map "email"
		map "userid", "id", int
