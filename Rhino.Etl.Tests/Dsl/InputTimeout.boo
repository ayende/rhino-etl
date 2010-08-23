process test_input_timeout:
	input "test", Command = "SELECT id, name as firstname, '' as lastname, email  FROM Users", Timeout = 60
	sqlBulkInsert "test", "People", TableLock = true :
		map "firstname"
		map "lastname"
		map "email"
		map "userid", "id", int
