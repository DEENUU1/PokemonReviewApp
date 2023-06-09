﻿using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PokemonReviewApp.Dto;
using PokemonReviewApp.Interfaces;
using PokemonReviewApp.Models;
using PokemonReviewApp.Repository;

namespace PokemonReviewApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OwnersController : Controller
    {

        private readonly IOwnerRepository _ownerRepository;
        private readonly ICountryRepository _countryRepository;
        private readonly IMapper _mapper; 

        public OwnersController(IOwnerRepository ownerRepository, ICountryRepository countryRepository, IMapper mapper)
        {
            _ownerRepository = ownerRepository;
            _countryRepository = countryRepository;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Owner>))]
        public IActionResult GetOwners()
        {
            var pokemons = _mapper.Map<List<OwnerDto>>(_ownerRepository.GetOwners());
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemons);
        }

        [HttpGet("{ownerId}")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetOwner(int pokeId)
        {
            if (!_ownerRepository.OwnerExists(pokeId))
                return NotFound();
            var pokemon = _mapper.Map<OwnerDto>(_ownerRepository.GetOwner(pokeId));
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(pokemon);
        }

        [HttpGet("{ownerId}/pokemon")]
        [ProducesResponseType(200, Type = typeof(Owner))]
        [ProducesResponseType(400)]
        public IActionResult GetPokemonByOwner(int ownerId)
        {
            if(!_ownerRepository.OwnerExists(ownerId))
                return NotFound();

            var owner = _mapper.Map<List<PokemonDto>>(_ownerRepository.GetOwner(ownerId));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(owner);

        }

        [HttpPost]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        public IActionResult CreateOwner([FromQuery] int countryId, [FromBody] OwnerDto createOwner)
        {
            if (createOwner == null)
                return BadRequest(ModelState);

            var owners = _ownerRepository.GetOwners()
                .Where(c => c.LastName.Trim().ToUpper() == createOwner.LastName.TrimEnd().ToUpper())
                .FirstOrDefault();

            if (owners != null)
            {
                ModelState.AddModelError("", "Owner already exists");
                return StatusCode(422, ModelState);
            }

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var ownerMap = _mapper.Map<Owner>(createOwner);

            ownerMap.Country = _countryRepository.GetCountry(countryId);

            if (!_ownerRepository.CreateOwner(ownerMap))
            {
                ModelState.AddModelError("", "Something went wrong while saving");
                return StatusCode(500, ModelState);
            }

            return Ok("Successfully created");

        }
    }
}
