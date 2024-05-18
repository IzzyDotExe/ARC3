import {TranscriptMemberComponent} from './TranscriptMemberComponent.jsx';
import {timeAgo} from '../lib/utils.js';

export  function TranscriptListItem({ data, key }) {

  const date = new Date(data.date);
  const type = data.transcripttype?? "Modmail";

  return (

    <a href={`/${data.GuildSnowflake}/transcripts/`+data.modmailId}>
      
      <div className="tr"  key={key}> 

        <p>{type} {timeAgo(date)}</p>
        
        <p>With</p>
        {
          data.participants.map((x, i, d) => {
            return <TranscriptMemberComponent userid={x} key={i}/>
          })
        }

      </div>
    </a>

  )
}
