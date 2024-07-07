import Infobox from "../Util/Infobox";
import Param from "../Text/Param";
import "./CommandStatInfoBox.css"

export default function CommandStatInfoBox({stats}) {
    return <Infobox>
        <h2>Recent Commands</h2>
        <div className="stat-infobox">
            {
                stats.length > 0 &&
                stats.slice(Math.max(stats.length - 5, 0)).map(x => {
                    let argsl = [];
                    for (let arg in x.args) {
                        argsl.push(<span><Param>{arg}</Param>:<Param>{x.args[arg]}</Param> </span>)
                    }
                    return <p>/{x.command_name} {argsl}</p>
                })
            }
            {
                stats.length === 0 &&
                <p>No recent commands</p>
            }
        </div>
    </Infobox>
}