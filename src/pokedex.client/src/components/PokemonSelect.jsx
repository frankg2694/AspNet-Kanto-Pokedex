export default function PokemonSelect({ pokemonList, selectedDexNum, onSelect, disabled = false }) {
  return (
    <select
      className="form-select mb-2"
      aria-label="Choose a Pokemon"
      value={selectedDexNum ?? ''}
      onChange={(e) => onSelect(Number(e.target.value))}
      disabled={disabled}
    >
      {/* Placeholder option, disabled so it can't be picked again */}
      <option value="" disabled>
        Choose a Pokemon...
      </option>
      {/* One option per Pokemon, key lets React tell them apart */}
      {pokemonList.map((pokemon) => (
        <option key={pokemon.dexNum} value={pokemon.dexNum}>
          {/* Pad the Dex Number with zeros, ex: 4 becomes #004 */}
          #{String(pokemon.dexNum).padStart(3, '0')} {pokemon.name}
        </option>
      ))}
    </select>
  )
}
