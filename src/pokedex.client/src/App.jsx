import PokemonGallery from './components/PokemonGallery'
import './App.css'

export default function App() {
  return (
    <main className="container py-4">
      <header className="mb-4">
        <h1 className="display-5 fw-bold">Kanto Pokedex</h1>
              <p className="text-body-secondary">A party of six from the original 151 Pokemon</p>
      </header>
      <PokemonGallery />
    </main>
  )
}
