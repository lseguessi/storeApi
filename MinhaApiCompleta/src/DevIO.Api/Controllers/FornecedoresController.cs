﻿using AutoMapper;
using DevIO.Api.ViewModels;
using DevIO.Business.Intefaces;
using DevIO.Business.Models;
using Microsoft.AspNetCore.Mvc;

namespace DevIO.Api.Controllers;

[Route("api/fornecedores")]
public class FornecedoresController : MainController
{
    private readonly IFornecedorRepository _fornecedorRepository;
    private readonly IFornecedorService _fornecedorService;
    private readonly IEnderecoRepository _enderecoRepository;
    private readonly IMapper _mapper;

    public FornecedoresController(IFornecedorRepository fornecedorRepository,
                                  IMapper mapper,
                                  IFornecedorService fornecedorService,
                                  INotificador notificador,
                                  IEnderecoRepository enderecoRepository) : base(notificador)
    {
        _fornecedorRepository = fornecedorRepository;
        _mapper = mapper;
        _fornecedorService = fornecedorService;
        _enderecoRepository = enderecoRepository;
    }

    [HttpGet]
    public async Task<IEnumerable<FornecedorViewModel>> ObterTodos()
    {
        var fornecedor = _mapper.Map<IEnumerable<FornecedorViewModel>>(await _fornecedorRepository.ObterTodos());

        return fornecedor;
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> ObterPorId(Guid id)
    {
        var fornecedor = await ObterForencerProdutosEndereco(id);

        if (fornecedor == null) return NotFound();

        return Ok(fornecedor);
    }

    [HttpGet("obter-endereco/{id:guid}")]
    public async Task<EnderecoViewModel> ObterEnderecoPorId(Guid id)
    {
        return _mapper.Map<EnderecoViewModel>(await _enderecoRepository.ObterPorId(id));
    }

    [HttpPost]
    public async Task<ActionResult<FornecedorViewModel>> Adicionar(FornecedorViewModel fornecedorViewModel)
    {
        if (!ModelState.IsValid) return CustomResponse(ModelState);

        await _fornecedorService.Adicionar(_mapper.Map<Fornecedor>(fornecedorViewModel));

        return CustomResponse(fornecedorViewModel);
    }

    [HttpPut("atualizar-endereco/{id:guid}")]
    public async Task<IActionResult> AtualizarEndereco(Guid id, EnderecoViewModel enderecoViewModel)
    {
        if(id != enderecoViewModel.Id)
        {
            NotificarErro("O id informado não é o mesmo que foi passado na query");
            return CustomResponse(enderecoViewModel);
        }

        if (!ModelState.IsValid) return CustomResponse(ModelState);

        await _fornecedorService.AtualizarEndereco(_mapper.Map<Endereco>(enderecoViewModel));

        return CustomResponse(enderecoViewModel);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> Atualizar(Guid id, FornecedorViewModel fornecedorViewModel)
    {
        if (id != fornecedorViewModel.Id)
        {
            NotificarErro("O id informado não é o mesmo que foi passado na query");
            return CustomResponse(fornecedorViewModel);
        }

        if (!ModelState.IsValid) return BadRequest();

        await _fornecedorService.Atualizar(_mapper.Map<Fornecedor>(fornecedorViewModel));

        return CustomResponse(fornecedorViewModel);
    }

    [HttpDelete("{id:guid}")]
    public async Task<ActionResult<FornecedorViewModel>> Excluir(Guid id)
    {
        var fornecedorViewModel = await ObterFornecedorEndereco(id);

        if (fornecedorViewModel == null) return NotFound();

        await _fornecedorService.Remover(id);

        return CustomResponse(fornecedorViewModel);
    }

    public async Task<FornecedorViewModel> ObterForencerProdutosEndereco(Guid id)
    {
        return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorProdutosEndereco(id));
    }

    public async Task<FornecedorViewModel> ObterFornecedorEndereco(Guid id)
    {
        return _mapper.Map<FornecedorViewModel>(await _fornecedorRepository.ObterFornecedorEndereco(id));
    }
}

