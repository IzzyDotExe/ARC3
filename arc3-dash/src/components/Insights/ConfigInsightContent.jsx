export default function ConfigInsightContent({ data }) {
  return <p><strong>{data.key}:</strong> {data.oldvalue} {'-->'} {data.newvalue}</p>
}
