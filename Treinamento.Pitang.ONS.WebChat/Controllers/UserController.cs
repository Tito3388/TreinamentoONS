﻿using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pitang.Treinamento.ONS.Data.Data;
using Pitang.Treinamento.ONS.Entities;
using Pitang.Treinamento.ONS.Services;
using Treinamento.Pitang.ONS.Views;

namespace Treinamento.Pitang.ONS.WebChat.Controllers
{
    [Route("user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMapper _mapper;
        private List<User> _users;

        public UserController(IMapper mapper)
        {
            _mapper = mapper;
            _users = new List<User>();
        }

        [HttpGet]
        [Route("")]
        public async Task<ActionResult<List<UserDto>>> Get([FromServices] DataContext contexto)
        {

           
            _users = await UserService.GetAllUsers(contexto);
            List<UserDto> usersDto = new List<UserDto>();
            foreach(User user in _users)
            {
                usersDto.Add(_mapper.Map<User, UserDto>(user));
            }

                return usersDto;
            
            
                //return BadRequest(new { message = "Não foi possível buscar os usuários" });
            
        }

        [HttpGet]
        [Route("{id:int}")]
        //[Authorize(Roles = "usuario")]
        public async Task<ActionResult<User>> GetById(
            int id,
            [FromServices] DataContext context)
        {
            try
            {
                var user = await UserService.GetUser(context, id);

                if (user == null)
                    return NotFound(new { message = "Usuário não encontrado" });

                return Ok(user);

            }
            catch
            {
                return BadRequest(new { message = "Não foi possível retornar o usuário" });
            }

        }

        [HttpPost]
        [Route("")]
        public async Task<ActionResult<User>> Post([FromBody] User model,
            [FromServices] DataContext context)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                context.Users.Add(model);
                await context.SaveChangesAsync();
                return Ok(model);

            }
            catch
            {
                return BadRequest(new { message = "Não foi possível criar o usuário" });

            }
        }

        [HttpPut]
        [Route("{id:int}")]
        //[Authorize(Roles = "usuario")]
        public async Task<ActionResult<User>> Put(
           int id,
           [FromBody] User model,
           [FromServices] DataContext context)
        {

            if (id != model.Id)
            {
                return NotFound(new { message = "Usuário não encontrado!" });
            }
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                context.Entry(model).State = EntityState.Modified;
                await context.SaveChangesAsync();
                return Ok(model);

            }
            catch (DbUpdateConcurrencyException)
            {
                return BadRequest(new { message = "Uma alteração já está sendo realizada" });
            }
            catch
            {
                return BadRequest(new { message = "Não foi possível alterar o usuário." });
            }
        }

        [HttpDelete]
        [Route("{id:int}")]
        //[Authorize(Roles = "admin")]
        public async Task<ActionResult<User>> Delete(
            int id,
            [FromServices] DataContext context
            )
        {
            var user = await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            if (user == null)
                return NotFound(new { message = "Usuário não encontrado" });

            try
            {
                context.Users.Remove(user);
                await context.SaveChangesAsync();

                return Ok(new { message = "Usuário deletado com sucesso!" });
            }
            catch
            {

                return BadRequest(new { message = "Não foi possível excluir o usuário." });
            }

        }

    }
}
