// connect to the databse 
const { MongoClient } =  require("mongodb");

const uri = process.env.MONGODB_URI
const client = new MongoClient(uri);

// remove all transcripts older than 30 days

async function run(collections) {

  const db = client.db('Arc3');

  const collection = db.collection("transcripts");

  // Delete all docs older than 30 days
  var del = await collection.deleteMany( { createdat : {"$lt" : new Date(Date.now() - 30*24*60*60 * 1000) } })
  if (del.acknowledged) {
    console.log(`Deleted ${del.deletedCount} records over 30 days old`);
  } else {
    console.log(`Delete not acknowledged`)
  }
  await client.close()
}

client.connect().then(x => {
  
  run().then(x => {
    
  }).catch(console.dir)
    
});
