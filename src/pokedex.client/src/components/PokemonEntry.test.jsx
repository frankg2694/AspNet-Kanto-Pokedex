import { describe, it, expect, vi } from 'vitest'
import { render, screen } from '@testing-library/react'
import PokemonEntry from './PokemonEntry'

// Fake fetch that always answers with the given data (like FakePokeApiHandler in the C# tests)
function fakeFetch(data, status = 200) {
    const fetch = vi.fn(async () => new Response(JSON.stringify(data), { status: status }))
    vi.stubGlobal('fetch', fetch)
    return fetch
}

describe('PokemonEntry', () => {
    // Check the Pokemon Card shows the Name, Dex Number, Type and Picture
    it('shows the name, number, type and picture of the Pokemon it loads', async () => {
        const fetch = fakeFetch({
            dexNum: 25,
            name: 'Pikachu',
            typeOne: 'Electric',
            typeTwo: null,
            picture: 'https://example.com/25.png',
        })

        render(<PokemonEntry pokemonList={[]} />)

        // findBy waits for the fake request to finish and the Card to update
        expect(await screen.findByRole('heading', { name: 'Pikachu #25' })).toBeInTheDocument()
        expect(screen.getByText('Electric')).toBeInTheDocument()
        expect(screen.getByAltText('Official artwork of Pikachu')).toHaveAttribute('src', 'https://example.com/25.png')
        expect(fetch).toHaveBeenCalledWith('/api/pokemon/random')
    })

    // Check the Pokemon Card only shows one badge when there is only one Type
    it('shows only one type badge when the Pokemon has one type', async () => {
        fakeFetch({
            dexNum: 4,
            name: 'Charmander',
            typeOne: 'Fire',
            typeTwo: null,
            picture: 'https://example.com/4.png',
        })

        const { container } = render(<PokemonEntry pokemonList={[]} />)

        await screen.findByRole('heading', { name: 'Charmander #4' })

        // An empty second badge would have no text to search for, so look for the badge class instead
        const badges = container.querySelectorAll('.badge')
        expect(badges).toHaveLength(1)
        expect(badges[0]).toHaveTextContent('Fire')
    })

    // Check the Card shows an error message when the request fails
    it('shows an error message when the API request fails', async () => {
        fakeFetch({}, 500)

        render(<PokemonEntry pokemonList={[]} />)

        const alert = await screen.findByRole('alert')
        expect(alert).toHaveTextContent('Could not load a Pokemon: Request failed with status 500')
        expect(screen.queryByRole('heading')).not.toBeInTheDocument()
    })
})