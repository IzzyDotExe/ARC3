import './ModmailInsightContent.css'
export default function ModmailInsightContent({ data }) {
  return <div>
    <p style={{textAlign: "center"}}><strong>ID: {data.mailid}</strong></p>
    <section className="is-modmail-content">
      <Stat value={data.messages} name="Messages"/>
      <Stat value={data.participants} name="Participants"/>
    </section>
  </div>

}

function Stat({value, name}) {
  return <section className="is-modmail-content-stat">
    <h2>{value}</h2>
    <p>{name}</p>
  </section>
}
