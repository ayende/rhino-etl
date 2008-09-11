namespace Rhino.Etl.Tests
{
    using System.Collections.Generic;
    using System.Data;
    using Aggregation;
    using Commons;
    using MbUnit.Framework;

    public class BaseUserToPeopleTest : BaseDslTest
    {
        [SetUp]
        public virtual void SetUp()
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
            Assert.AreEqual("ayende", names[0][0]);
            Assert.AreEqual("rahien", names[0][1]);
            Assert.AreEqual("foo", names[1][0]);
            Assert.AreEqual("bar", names[1][1]);
            Assert.AreEqual("nice", names[2][0]);
            Assert.AreEqual("naughty", names[2][1]);
            Assert.AreEqual("gold", names[3][0]);
            Assert.AreEqual("silver", names[3][1]);
        }
    }
}