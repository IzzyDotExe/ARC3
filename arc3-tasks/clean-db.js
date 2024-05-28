// Connect to the mongo database
const { MongoClient } =  require("mongodb");
const debug = process.env.DEBUG;

const uri = process.env.MONGODB_URI;
var it = [ ... process.argv ] 

if (debug === 'true')
  console.log(it)

it.shift()
it.shift()

if (debug === 'true')
  console.log(it)

const client = new MongoClient(uri);

async function run(collections) {

    const db = client.db('Arc3');
    let proc = 0;
    // Go through all the tables that have data that is temp (Modmails, Jails, Appeals, Karaoke)
    collections.forEach(async x => {

      const collection = db.collection(x);

      // Clean out all of that data.
      var dr = await collection.drop();
      if (dr) {
        console.log(`Successfully cleaned collection: ${x}`);
      }

      proc++;

      if (proc === collections.length) {
        console.log("DB cleaned, closing")
        await client.close();
      }
    })

}

client.connect().then(x => {
  
  run(it).then(x => {
    
  }).catch(console.dir)
    
});



