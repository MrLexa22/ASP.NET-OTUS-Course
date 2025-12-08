import { useState, useEffect } from 'react'
import reactLogo from './assets/react.svg'
import viteLogo from '/vite.svg'
import './App.css'

type WeatherForecast = {
  date: string
  temperatureC: number
  temperatureF: number
  summary: string
}

function App() {
  const [count, setCount] = useState(0);

  const [weather, setWeather] = useState<WeatherForecast[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    fetch('/WeatherForecast')
      .then(res => res.json())
      .then(data => {
        setWeather(data)
        setLoading(false)
      })
      .catch(() => setLoading(false))
  }, []);

  return (
    <>
      <div>
        <a href="https://vite.dev" target="_blank">
          <img src={viteLogo} className="logo" alt="Vite logo" />
        </a>
        <a href="https://react.dev" target="_blank">
          <img src={reactLogo} className="logo react" alt="React logo" />
        </a>
      </div>
      <h1>Vite + React</h1>
      <div className="card">
        <button onClick={() => setCount((count) => count + 1)}>
          count is {count}
        </button>
        <p>
          Edit <code>src/App.tsx</code> and save to test HMR
        </p>
      </div>
      <p className="read-the-docs">
        Click on the Vite and React logos to learn more
      </p>

      <br></br>

      <h2>Weather Forecast</h2>
      {loading && <p>Loading weather...</p>}
      {!loading && (
        <table cellPadding={8} style={{ borderCollapse: 'collapse', marginTop: '1rem' }}>
          <thead>
            <tr>
              <th>Дата</th>
              <th>Температура (C)</th>
              <th>Температура (F)</th>
              <th>Инфо</th>
            </tr>
          </thead>
          <tbody>
            {weather.map((w, i) => (
              <tr key={i}>
                <td>{w.date}</td>
                <td>{w.temperatureC}</td>
                <td>{w.temperatureF}</td>
                <td>{w.summary}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
    </>
  )
}

export default App
