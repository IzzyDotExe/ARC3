// Connect to the mongo database
const { MongoClient } =  require("mongodb");
const tar = require('tar');
const fs = require('fs');
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
    const files = []
    let proc = 0;

    // Go through all the tables that have data that is temp (Modmails, Jails, Appeals, Karaoke)
    collections.forEach(async x => {
      const collection = db.collection(x);
      

      const data = await collection.find();
      const items = []
      for await ( let item of data) {
        
        if (debug === 'true')
          console.log(item)

        items.push(item)
      }

      const json = JSON.stringify(items);

      fs.writeFile(`./bkup/${x}.json`, json, 'utf8', async () => {

        proc++;

        if (proc === collections.length) {
          tar.create(
            { file: `./out/backup-${Date.now()}.tar.gz`, gzip: true },
            ['./bkup']
          ).then(async _ => {
            await client.close()
            console.log(`Saved ${proc} collections`)
          })
        }

      })      




    })



}

client.connect().then(x => {
  
  run(it).then(x => {

  }).catch(console.dir)
    
});



