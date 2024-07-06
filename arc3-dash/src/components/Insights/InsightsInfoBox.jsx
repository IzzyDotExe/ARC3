import InfoBox from '../Util/Infobox.jsx'
import './InsightsInfoBox.css'
import Insight from './Insight.jsx'
import NoInsights from './NoInsights'

import axios from 'axios'

import { getInsightContent } from './InsightContentTypes.js'

import { useState, useEffect, useCallback } from 'react'

export default function GuildInfoBox({guild}) {

  const [insights, setInsights] = useState([])

  const insightsBox = useCallback(() => {

    if (insights.length === 0)
      return <NoInsights/>

    const insightElements = []
    
    insights.sort((a, b) => {
      return parseInt(b.date) - parseInt(a.date)
    }).forEach(insight => {
      
      let insightcontent = getInsightContent(insight)
      
      insightElements.push(
        <Insight insight={insight}>
          {insightcontent}
        </Insight>
      )
    })
    
    return insightElements
  }, [insights]);

  useEffect(() => {

    if (guild.id)
      axios.get(`/api/insights?guildid=${guild.id}`).then(res => {
        setInsights(res.data)
        console.log(res.data)
      })

  }, [guild.id])

  return <InfoBox>
    <h1>Insights</h1>
    <div className="insights-infobox">
      {insightsBox()}
    </div>
  </InfoBox>
}