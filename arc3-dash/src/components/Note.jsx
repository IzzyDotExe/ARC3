import {timeAgo} from '../lib/utils.js';
import {TranscriptMemberComponent} from './TranscriptMemberComponent.jsx';

export function Note({ data, key }) {

  const date = new Date(data.date*1000);

  return (
      <div className="tr" style={{"background-color": "gray"}}  key={key}> 

        <p>{timeAgo(date)}</p>
        <p style= {{"background-color": "lightgray", "padding": "10px", "border": "black solid 2px", "color": "black"}}>{data.note}</p>
        <p>Added by</p>      
        <TranscriptMemberComponent userid={data.authorsnowflake}/>

      </div>

  )
}
