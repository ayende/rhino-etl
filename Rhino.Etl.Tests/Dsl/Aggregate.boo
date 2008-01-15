aggregate distinct_product_names:
	groupBy name
	accumulate:
		aggregate.name = row.name
		
aggregate join_product_names:
	accumulate:
		aggregate.names = [] if aggregate.names is null
		aggregate.names.Add(row.name)
	
	terminate:
		aggregate.result = string.Join(", ", aggregate.names.ToArray(string))
		
process aggregate_names:
	# we get the products from the unit test
	distinct_product_names()
	join_product_names()
	# we output the result to the unit test