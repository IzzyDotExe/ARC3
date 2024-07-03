

export default function Navbar({guild}) {
  return (
    <nav>
      <h2>Arc V3</h2>
      <ul>

        <li><a href="/">Home</a></li>
        {/* <li><a href='/appeal'>Appeal</a></li>
        <li><a href='/appeals'>Appeals</a></li> */}
        {
          guild && <li><a href={`/${guild}/transcripts/`}>Transcripts</a></li>
        }
        {
          guild &&
          <li><a href={`/${guild}/notes`}>User Notes</a></li>
        }
      </ul>
    </nav>
  )
}