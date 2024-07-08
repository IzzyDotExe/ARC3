import {Note} from "./Note";
import "./NotesDisplay.css"

export default function NotesDisplay({notes, filter}) {

    return <section className="notes-display">
        {
            notes.sort((a, b) => {
                return parseInt(b.date) - parseInt(a.date)
            }).map((x, i) => {
                return <Note note={x} lid={i} key={i}/>
            })
        }
    </section>
}