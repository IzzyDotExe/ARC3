import Param from "../Text/Param";
import './ConfigInsightContent.css'

export default function ConfigInsightContent({ data }) {
  return <p className="config-insight"><strong>{data.key}:</strong> <Param>{data.oldvalue}</Param> {'-->'} <Param>{data.newvalue}</Param></p>
}
