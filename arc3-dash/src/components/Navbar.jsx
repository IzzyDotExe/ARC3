import './Navbar.css';

export default function Navbar() {
  return (
    <nav>
      <h2>Arc V3</h2>
      <ul>
        <li><a href="/">Home</a></li>
        {/* <li><a href='/appeal'>Appeal</a></li>
        <li><a href='/appeals'>Appeals</a></li> */}
        <li><a href="/transcripts/">Transcripts</a></li>
        <li><a href="/569929112932712469/notes">User Notes</a></li>
      </ul>
    </nav>
  )
}