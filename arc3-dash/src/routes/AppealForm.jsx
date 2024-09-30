
import "./AppealForm.css"
import {Outlet} from "react-router-dom";
import GuildAppeal from "../components/GuildAppeal";

import {useEffect, useState} from "react";

import axios from 'axios';
import {useParams} from "react-router";

export default function AppealForm() {

    // Get the ID of the modmail
    const {guildid} = useParams();

    const [guild, setGuild] = useState(null);

    useEffect(() => {

        // Fetch the guild
        axios.get(`/api/discord/guilds/${guildid}/`).then(res => {
            setGuild(res.data);
        })

    }, [guildid]);

    const GuildIcon = guild? `https://cdn.discordapp.com/icons/${guild.id}/${guild.icon}.png?size=128` : "/missing.jpg";
    const ServerName = guild? guild.name : "GuildName";

    return (
        <div className="appeal-form">
            <section className="form">
                <section className="header-sec">
                    <img src={GuildIcon} alt="GuildIcon" />
                    <h2>{ServerName}</h2>
                </section>
                <GuildAppeal guild={guild}/>
            </section>
        </div>
    )

}


