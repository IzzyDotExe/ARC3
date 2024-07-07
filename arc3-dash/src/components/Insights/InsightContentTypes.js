import ModmailInsightContent from './ModmailInsightContent.jsx'
import ConfigInsightContent from './ConfigInsightContent.jsx'

export function getInsightContent( insight ) {
  console.log(insight)
  switch (insight.type) {
    case "modmail":
      return <ModmailInsightContent data={insight.data}/>
    case "config":
      return <ConfigInsightContent data={insight.data}/>
    default: 
      return <p>No insight content was found...</p>
  }
}