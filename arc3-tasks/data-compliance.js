// connect to the databse 
const { MongoClient } =  require("mongodb");

const uri = process.env.MONGODB_URI
const client = new MongoClient(uri);

// remove all transcripts older than 30 days

async function run(collections) {

  const db = client.db('Arc3');

  const collection = db.collection("transcripts");

  // Delete all docs older than 30 days
  collection.deleteMany( { createdat : {"$lt" : new Date(Date.now() - 30*24*60*60 * 1000) } })

}

client.connect().then(x => {
  
  run().then(x => {

    setTimeout(async () => {
      await client.close()
    }, 3000)
    
  }).catch(console.dir)
    
});
