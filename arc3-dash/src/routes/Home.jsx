
import { useParams } from 'react-router';

export default function Home() {

  const {guildid} = useParams()

  return (
    <div className="App">
      <main>
        <h1>{guildid}</h1>
      </main>
    </div>
  );
};