import Navbar from '../components/Navbar'

export default function Appeal() {
    return (
        <>
            
            <div className="App">
                <Navbar/>
                <main>
                    <form action='/api/appeals' method="POST">
                        
                        <input id="bannedBy" name="bannedBy" placeholder="Who were you banned by?"/>
                        <textarea name="appealContent" id="appealContent" cols="30" rows="10"></textarea>
                        <button type="submit">Submit</button>

                    </form>
                </main>
            </div>
        
        </>
    )
}