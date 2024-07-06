
export default function IconLabel({tag, src}) {
  return (<>
    <img draggable="false" src={src} alt="" />
    <p>{tag}</p>
  </>)
}