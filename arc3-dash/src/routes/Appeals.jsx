import Navbar from '../components/Navbar'

import { useState, useEffect } from 'react' 
import axios from 'axios'

import Appeal from '../components/Appeal'

export default function Appeals({ self }) {

  const [appeals, setAppeals] = useState([]);

  useEffect(() => {
    axios.get('/api/appeals').then(res => {
      setAppeals(res.data)
    })
  }, [setAppeals])

  return (
    <>
      
      <div className="App">
        <Navbar/>
        {/* <main className="appeals">
          {appeals.map(x => <Appeal self={self} data={x} />)}
        </main> */}
        <h1>This feature is not yet enabled!</h1>
      </div>
    
    </>
  )
}