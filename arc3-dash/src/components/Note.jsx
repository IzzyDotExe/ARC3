import {timeAgo} from '../lib/utils.js';
import {TranscriptMemberComponent} from './TranscriptMemberComponent.jsx';

export function Note({ data, key }) {

  const date = new Date(data.date*1000);

  return (
      <div className="tr"  key={key}> 

        <p>{timeAgo(date)}</p>
        <p>{data.note}</p>
        <p>Added by</p>      
        <TranscriptMemberComponent userid={data.authorsnowflake}/>

      </div>

  )
}
