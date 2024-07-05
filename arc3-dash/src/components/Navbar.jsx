import { useEffect, useState } from 'react'
import MemberLabel from './MemberLabel.jsx'
import './Navbar.css'

export default function Navbar({tag, guild, self}) {

  return (
    <nav className="nav-bar">
      <div className="labels">
        <h2>{tag}</h2>
      </div>
      <MemberLabel placement="left" user={self} />
    </nav>
  )
}