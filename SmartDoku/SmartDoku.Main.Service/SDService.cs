﻿using SmartDoku.Main.IService;
using SmartDoku.Main.Model;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SmartDoku.Main.Service
{
  public class SDService : ISDService
  {
    public void Dispose()
    {
      //this.Dispose();
    }

    public void AlteraValorCelula(SDMatrizModel matriz, SDCelulaModel celulaAlterada, SDCelulaModel oldCelula)
    {
      AjustaStatusCelula(matriz, celulaAlterada);
      ReavaliaCelulasAposAlteracao(matriz, oldCelula);
    }

    public void ReavaliaCelulasAposAlteracao(SDMatrizModel matriz, SDCelulaModel oldCelula)
    {
      var listaCelulasParaReavaliar = new List<SDCelulaModel>();

      listaCelulasParaReavaliar.AddRange(matriz.Linhas.Find(linha => linha.NumeroSequencial == oldCelula.PosicaoLinha)
                                                      .Celulas.Where(celula => celula.Valor == oldCelula.Valor).ToList());
      listaCelulasParaReavaliar.AddRange(matriz.Colunas.Find(coluna => coluna.NumeroSequencial == oldCelula.PosicaoColuna)
                                                      .Celulas.Where(celula => celula.Valor == oldCelula.Valor).ToList());
      listaCelulasParaReavaliar.AddRange(matriz.Quadrantes.Find(quadrante => quadrante.Celulas.Contains(oldCelula))
                                                      .Celulas.Where(celula => celula.Valor == oldCelula.Valor).ToList());

      foreach (var celulaParaReavaliar in listaCelulasParaReavaliar)
      {
        AjustaStatusCelula(matriz, celulaParaReavaliar);
      }
    }

    public void AjustaStatusCelula(SDMatrizModel matriz, SDCelulaModel celulaAlterada)
    {
      var listaCelulasInvalidas = RetornaListaCelulasInvalidas(matriz, celulaAlterada);

      if (listaCelulasInvalidas.Any())
        celulaAlterada.isCelulaValida = false;
      else
        celulaAlterada.isCelulaValida = true;

      foreach (var celulaInvalida in listaCelulasInvalidas)
      {
        celulaInvalida.isCelulaValida = false;
      }
    }

    public void AjustaStatusCelula(SDMatrizModel matriz, List<SDCelulaModel> listCelulasInvalidas)
    {
      foreach (var celulaInvalida in listCelulasInvalidas)
      {
        celulaInvalida.isCelulaValida = false;
      }
    }

    private List<SDCelulaModel> RetornaListaCelulasInvalidas(SDMatrizModel matriz, SDCelulaModel celulaAlterada)
    {
      List<SDCelulaModel> listaCelulasInvalidas = new List<SDCelulaModel>();

      listaCelulasInvalidas.AddRange(matriz.Linhas.
          Find(linha => linha.NumeroSequencial == celulaAlterada.PosicaoLinha).
            Celulas.Where(celula => celula.PosicaoColuna != celulaAlterada.PosicaoColuna
                                          && celula.Valor == celulaAlterada.Valor
                                          && (celula.Valor > 0 && celula.Valor != null)).ToList());

      listaCelulasInvalidas.AddRange(matriz.Colunas.
              Find(coluna => coluna.NumeroSequencial == celulaAlterada.PosicaoColuna).
                Celulas.Where(celula => celula.PosicaoLinha != celulaAlterada.PosicaoLinha
                                              && celula.Valor == celulaAlterada.Valor
                                              && (celula.Valor > 0 && celula.Valor != null)).ToList());

      listaCelulasInvalidas.AddRange(matriz.Quadrantes.
        Find(quadrante => quadrante.Celulas.Contains(celulaAlterada)).
          Celulas.Where(celula => celula.PosicaoLinha != celulaAlterada.PosicaoLinha
                               && celula.PosicaoColuna != celulaAlterada.PosicaoColuna
                               && celula.Valor == celulaAlterada.Valor
                               && (celula.Valor >0 && celula.Valor != null)).ToList());

      return listaCelulasInvalidas;
    }

    public void GeraDigitosIniciais(SDMatrizModel matriz, int qtdeDigitosIniciais)
    {
      LimpaValoresCelulas(matriz);

      int digGerados = 0;

      if (qtdeDigitosIniciais > 0)
      {
        do
        {
          var achou = false;
          SDCelulaModel cel = new SDCelulaModel();

          while (!achou)
          {
            var posLinha = new Random().Next(1, 10);
            var posCol = new Random().Next(1, 10);

            achou = matriz.Celulas.Exists(celula => celula.PosicaoLinha == posLinha
                                                 && celula.PosicaoColuna == posCol
                                                 && (celula.Valor == null || celula.Valor == 0));
            if (achou)
              cel = matriz.Celulas.Find(celula => celula.PosicaoLinha == posLinha
                                               && celula.PosicaoColuna == posCol);
          }

          do
          {
            cel.Valor = new Random().Next(1, 10);

          } while (RetornaListaCelulasInvalidas(matriz, cel).Any());

          digGerados++;

        } while (digGerados < qtdeDigitosIniciais);
      }
    }


    private void LimpaValoresCelulas(SDMatrizModel matriz)
    {
      matriz.Celulas.ForEach(celula => celula.Valor = null);
    }
  }
}
