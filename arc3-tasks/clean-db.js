// Connect to the mongo database
const { MongoClient } =  require("mongodb");

const uri = process.env.MONGODB_URI
console.log(uri)

const client = new MongoClient(uri);

async function run(collections) {

  try {
    await client.connect();

    const db = client.db('Arc3');

    // Go through all the tables that have data that is temp (Modmails, Jails, Appeals, Karaoke)
    collections.forEach(async x => {
      const collection = db.collection(x);

      // Clean out all of that data.
      await collection.drop();

    })

  } finally {
    await client.close()
  }

}

run(["jails"]).catch(console.dir)



