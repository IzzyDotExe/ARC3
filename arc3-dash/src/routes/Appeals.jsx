import Navbar from '../components/Navbar'

import { useState, useEffect } from 'react' 
import axios from 'axios'

import Appeal from '../components/Appeal'


export default function Appeals() {

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
              <main>
                {appeals.map(x => <Appeal data={x} />)}
              </main>
          </div>
      
      </>
  )
}