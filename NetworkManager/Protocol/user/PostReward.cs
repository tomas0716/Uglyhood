﻿/**
 * Autogenerated by Thrift Compiler (0.9.1)
 *
 * DO NOT EDIT UNLESS YOU ARE SURE THAT YOU KNOW WHAT YOU ARE DOING
 *  @generated
 */
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Thrift;
using Thrift.Collections;
using System.Runtime.Serialization;
using Thrift.Protocol;
using Thrift.Transport;

namespace sg.protocol.user
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class PostReward : TBase
  {
    private int _i_FreeGem;
    private int _i_PaymentGem;
    private int _i_Gold;
    private List<sg.protocol.common.Items> _l_Items;

    public int I_FreeGem
    {
      get
      {
        return _i_FreeGem;
      }
      set
      {
        __isset.i_FreeGem = true;
        this._i_FreeGem = value;
      }
    }

    public int I_PaymentGem
    {
      get
      {
        return _i_PaymentGem;
      }
      set
      {
        __isset.i_PaymentGem = true;
        this._i_PaymentGem = value;
      }
    }

    public int I_Gold
    {
      get
      {
        return _i_Gold;
      }
      set
      {
        __isset.i_Gold = true;
        this._i_Gold = value;
      }
    }

    public List<sg.protocol.common.Items> L_Items
    {
      get
      {
        return _l_Items;
      }
      set
      {
        __isset.l_Items = true;
        this._l_Items = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_FreeGem;
      public bool i_PaymentGem;
      public bool i_Gold;
      public bool l_Items;
    }

    public PostReward() {
      this._i_FreeGem = 0;
      this.__isset.i_FreeGem = true;
      this._i_PaymentGem = 0;
      this.__isset.i_PaymentGem = true;
      this._i_Gold = 0;
      this.__isset.i_Gold = true;
    }

    public void Read (TProtocol iprot)
    {
      TField field;
      iprot.ReadStructBegin();
      while (true)
      {
        field = iprot.ReadFieldBegin();
        if (field.Type == TType.Stop) { 
          break;
        }
        switch (field.ID)
        {
          case 1:
            if (field.Type == TType.I32) {
              I_FreeGem = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              I_PaymentGem = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_Gold = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.List) {
              {
                L_Items = new List<sg.protocol.common.Items>();
                TList _list0 = iprot.ReadListBegin();
                for( int _i1 = 0; _i1 < _list0.Count; ++_i1)
                {
                  sg.protocol.common.Items _elem2 = new sg.protocol.common.Items();
                  _elem2 = new sg.protocol.common.Items();
                  _elem2.Read(iprot);
                  L_Items.Add(_elem2);
                }
                iprot.ReadListEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          default: 
            TProtocolUtil.Skip(iprot, field.Type);
            break;
        }
        iprot.ReadFieldEnd();
      }
      iprot.ReadStructEnd();
    }

    public void Write(TProtocol oprot) {
      TStruct struc = new TStruct("PostReward");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_FreeGem) {
        field.Name = "i_FreeGem";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_FreeGem);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_PaymentGem) {
        field.Name = "i_PaymentGem";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_PaymentGem);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_Gold) {
        field.Name = "i_Gold";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Gold);
        oprot.WriteFieldEnd();
      }
      if (L_Items != null && __isset.l_Items) {
        field.Name = "l_Items";
        field.Type = TType.List;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteListBegin(new TList(TType.Struct, L_Items.Count));
          foreach (sg.protocol.common.Items _iter3 in L_Items)
          {
            _iter3.Write(oprot);
          }
          oprot.WriteListEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("PostReward(");
      sb.Append("I_FreeGem: ");
      sb.Append(I_FreeGem);
      sb.Append(",I_PaymentGem: ");
      sb.Append(I_PaymentGem);
      sb.Append(",I_Gold: ");
      sb.Append(I_Gold);
      sb.Append(",L_Items: ");
      sb.Append(L_Items);
      sb.Append(")");
      return sb.ToString();
    }

  }

}