using Rhino.Etl.Core.Infrastructure;

namespace Rhino.Etl.Tests
{
    using System.Collections.Generic;
    using System.Data;
    using Aggregation;
    using Xunit;

    public class BaseUserToPeopleTest : BaseDslTest
    {
        public BaseUserToPeopleTest()
        {
            Use.Transaction("test", delegate(IDbCommand cmd)
            {
                cmd.CommandText =
					@"
if object_id('User2Role') is not null
    drop table User2Role;
if object_id('Roles') is not null
    drop table Roles;
if object_id('Users') is not null
    drop table Users;

create table Users ( id int identity primary key, name nvarchar(255) not null, email nvarchar(255) not null, roles nvarchar(255), testMsg nvarchar(255) );
create table Roles( id int identity, name nvarchar(255) );


create table User2Role( userid int, roleid int);

insert into users (name,email) values('ayende rahien', 'ayende@example.org')
insert into users (name,email) values('foo bar', 'fubar@example.org')
insert into users (name,email) values('nice naughty', 'santa@example.org')
insert into users (name,email) values('gold silver', 'dwarf@example.org')


insert into roles values('admin')
insert into roles values('janitor')
insert into roles values('employee')
insert into roles values('customer')

insert into User2Role values(1,1)
insert into User2Role values(1,2)
insert into User2Role values(1,3)
insert into User2Role values(1,4)
insert into User2Role values(2,2)
insert into User2Role values(4,2)
insert into User2Role values(4,3)

if object_id('People') is not null
    drop table People;
create table People ( id int identity, userid int not null, firstname nvarchar(255) not null, lastname nvarchar(255) not null, email nvarchar(255) not null);
";
                cmd.ExecuteNonQuery();
            });
        }

        protected static void AssertNames(IList<string[]> names)
        {
            Assert.Equal("ayende", names[0][0]);
            Assert.Equal("rahien", names[0][1]);
            Assert.Equal("foo", names[1][0]);
            Assert.Equal("bar", names[1][1]);
            Assert.Equal("nice", names[2][0]);
            Assert.Equal("naughty", names[2][1]);
            Assert.Equal("gold", names[3][0]);
            Assert.Equal("silver", names[3][1]);
        }

        protected static void AssertFullNames(IList<string> names)
        {
            Assert.Equal("ayende rahien", names[0]);
            Assert.Equal("foo bar", names[1]);
            Assert.Equal("nice naughty", names[2]);
            Assert.Equal("gold silver", names[3]);
        }
    }
}