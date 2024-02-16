# Database

I am experimenting with creating a database. It will be based around embedded
replicas, horizontal scaling and having direct access to data from programming
language instead of separate query language.

Don't have particular name for it, here are couple ones:

- Unator DB
- Folder DB
- Embedded DB

Other databases, which inspire creation:

- [Turso](https://turso.tech/)
- [SpacetimeDB](https://spacetimedb.com/)

## Overview

Singe instance of server with experimental database:

![Instance of server](../media/instance.png)

As you can see, your application logic and database logic are combined into 1
server. It allows to not have a Query Language layer as well as network overhead.

## Horizontal Scaling

Because our database is based around embedded model, we must handle horizontal
scaling on database level.

Keep in mind that it's possible to not have embedded database, but just send
request to the server that has one. But let's not care about it for now.

Here is simple scheme for you:

![Many instances](../media/many-instances.png)

## Storing data

I don't plan to keep all data in memory, because it's too expensive.
Of course, if you have certain requirments, you could store whole db
inside of RAM.

But for most of edge applications it's not required. Maybe I will support it
as an optional, but for now I will focuse on how to store it on the disk.

Because even if all our data is in RAM, we still need optimized way to
back up it into file system to not loose data after redeploy.

During UPDATE or DELETE shifting of data to right or left is too expensive.
Because number of bytes to shift may be really big. Imaging shifting 2GB!

So I want to just mark space as "Cleaned up" and than later shift it by small
steps, so we can space for new records:

![Inserting new record](../media/storing-data.png)

With such model we _MUST NOT EXPECT_ that queried data will have the same order
as we added it. Because if new record is small enough to be inserted into
"Cleaned up" space, then it will be.
