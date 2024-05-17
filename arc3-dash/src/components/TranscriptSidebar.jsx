import {TranscriptListItem} from './TranscriptListItem.jsx';

export function TranscriptSidebar({transcripts, toggleSidebar}) {
  return (
    <div className="side-bar">
      <button onClick={toggleSidebar}>Close</button>
      <h1>Transcripts</h1>
      {transcripts.sort((a,b) => {
        return new Date(b.date) - new Date(a.date);
      }).map((x, i, a) =>
        <TranscriptListItem data={x} key={i}/>
      )}
    </div>
  )
}