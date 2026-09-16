import React, { useState, useEffect } from 'react'; 
 
interface User { id: string; username: string; email: string; } 
interface Message { id: string; senderId: string; receiverId: string; content: string; sentAt: string; } 
 
export default function App() { 
  const [users, setUsers] = useState<User[]>([]); 
  const [currentUser, setCurrentUser] = useState<User | null>(null); 
  const [selectedUser, setSelectedUser] = useState<User | null>(null); 
  const [messages, setMessages] = useState<Message[]>([]); 
  const [newMsg, setNewMsg] = useState(''); 
 
  useEffect(() => { 
    fetch('http://localhost:5000/api/users') 
      .then(res => res.json()) 
      .then(data => { 
        setUsers(data); 
        if (data.length > 0) setCurrentUser(data[0]); 
      }); 
  }, []); 
 
  useEffect(() => { 
    if (currentUser && selectedUser) { 
      fetch(`http://localhost:5000/api/messages/${currentUser.id}/${selectedUser.id}`) 
        .then(res => res.json()) 
        .then(data => setMessages(data)); 
    } 
  }, [currentUser, selectedUser]); 
 
  const sendMessage = async () => { 
    if (!currentUser || !selectedUser || !newMsg) return; 
     
    const payload = { 
      senderId: currentUser.id, 
      receiverId: selectedUser.id, 
      content: newMsg 
    }; 
 
    const res = await fetch('http://localhost:5000/api/messages', { 
      method: 'POST', 
      headers: { 'Content-Type': 'application/json' }, 
      body: JSON.stringify(payload) 
    }); 
 
    if (res.ok) { 
      const msg = await res.json(); 
      setMessages([...messages, msg]); 
      setNewMsg(''); 
    } else { 
      alert('Error: Asegúrate de estar conectado con esta persona.'); 
    } 
  }; 
 
  return ( 
    <div style={{ display: 'flex', gap: '20px', padding: '20px', fontFamily: 'sans-serif' }}> 
      <div style={{ width: '250px', borderRight: '1px solid #ccc' }}> 
        <h3>Usuario Actual</h3> 
        <select onChange={e => setCurrentUser(users.find(u => u.id === e.target.value) || null)}> 
          {users.map(u => <option key={u.id} value={u.id}>{u.username}</option>)} 
        </select> 
 
        <h3>Personas</h3> 
        {users.filter(u => u.id !== currentUser?.id).map(u => ( 
          <div  
            key={u.id}  
            onClick={() => setSelectedUser(u)}  
            style={{ padding: '8px', cursor: 'pointer', background: selectedUser?.id === u.id ? '#ddd' : 
'transparent' }} 
          > 
            {u.username} 
          </div> 
        ))} 
      </div> 
 
      <div style={{ flex: 1 }}> 
        {selectedUser ? ( 
          <> 
            <h3>Chat con {selectedUser.username}</h3> 
            <div style={{ height: '300px', overflowY: 'scroll', border: '1px solid #eee', padding: '10px' 
}}> 
              {messages.map(m => ( 
                <div key={m.id} style={{ textAlign: m.senderId === currentUser?.id ? 'right' : 'left' }}> 
                  <p style={{ display: 'inline-block', padding: '8px', background: m.senderId === 
currentUser?.id ? '#007bff' : '#f1f1f1', color: m.senderId === currentUser?.id ? '#fff' : '#000', 
borderRadius: '8px' }}> 
                    {m.content} 
                  </p> 
                </div> 
              ))} 
            </div> 
            <div style={{ marginTop: '10px' }}> 
              <input value={newMsg} onChange={e => setNewMsg(e.target.value)} 
placeholder="Escribe un mensaje..." /> 
              <button onClick={sendMessage}>Enviar</button> 
            </div> 
          </> 
        ) : <p>Selecciona un usuario para conversar</p>} 
      </div> 
    </div> 
  ); 
} 
 

