import { useEffect, useState } from 'react'
import { fetchPokemonList } from '../api/pokemon'
import PokemonEntry from './PokemonEntry'

export default function PokemonGallery() {
    // State of the Gallery: the Pokemon list for the dropdowns, if it is loading, and the error message if any
    const [pokemonList, setPokemonList] = useState([])
    const [loading, setLoading] = useState(true) // Controls the loading spinner
    const [error, setError] = useState(null)

    // Load the list of 151 Pokemon once, and share it with every Card (PokemonEntry)
    useEffect(() => {
        (async () => {
            try {
                const result = await fetchPokemonList()
                setPokemonList(result)
            } catch (err) {
                setError(err.message)
            } finally {
                setLoading(false) // Stop loading even if there was an error
            }
        })()

    }, [])

    return (
        <>
            {error && (
                <div className="alert alert-warning" role="alert">
                    Could not load the Pokemon list: {error}
                </div>
            )}

            {/* Show a spinner until the list is loaded, then the Party of Cards */}
            {loading ? (
                <div className="d-flex justify-content-center py-5">
                    <div className="spinner-border" role="status">
                        <span className="visually-hidden">Loading...</span>
                    </div>
                </div>
            ) : (
                <div className="row g-4">
                    {/* Make six Cards */}
                    {Array.from({ length: 6 }, (_, i) => (
                        <div key={i} className="col-12 col-sm-6 col-lg-4">
                            <PokemonEntry pokemonList={pokemonList} />
                        </div>
                    ))}
                </div>
            )}
        </>
    )
}
