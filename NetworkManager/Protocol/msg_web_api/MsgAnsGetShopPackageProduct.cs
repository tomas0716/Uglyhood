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

namespace sg.protocol.msg_web_api
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class MsgAnsGetShopPackageProduct : TBase
  {
    private sg.protocol.basic.Error _error;
    private Dictionary<int, sg.protocol.common.ShopPackage> _m_ShopPackage;
    private Dictionary<int, sg.protocol.common.ShopPackageGoods> _m_ShopPackageGoods;

    public sg.protocol.basic.Error Error
    {
      get
      {
        return _error;
      }
      set
      {
        __isset.error = true;
        this._error = value;
      }
    }

    public Dictionary<int, sg.protocol.common.ShopPackage> M_ShopPackage
    {
      get
      {
        return _m_ShopPackage;
      }
      set
      {
        __isset.m_ShopPackage = true;
        this._m_ShopPackage = value;
      }
    }

    public Dictionary<int, sg.protocol.common.ShopPackageGoods> M_ShopPackageGoods
    {
      get
      {
        return _m_ShopPackageGoods;
      }
      set
      {
        __isset.m_ShopPackageGoods = true;
        this._m_ShopPackageGoods = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool error;
      public bool m_ShopPackage;
      public bool m_ShopPackageGoods;
    }

    public MsgAnsGetShopPackageProduct() {
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
            if (field.Type == TType.Struct) {
              Error = new sg.protocol.basic.Error();
              Error.Read(iprot);
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.Map) {
              {
                M_ShopPackage = new Dictionary<int, sg.protocol.common.ShopPackage>();
                TMap _map125 = iprot.ReadMapBegin();
                for( int _i126 = 0; _i126 < _map125.Count; ++_i126)
                {
                  int _key127;
                  sg.protocol.common.ShopPackage _val128;
                  _key127 = iprot.ReadI32();
                  _val128 = new sg.protocol.common.ShopPackage();
                  _val128.Read(iprot);
                  M_ShopPackage[_key127] = _val128;
                }
                iprot.ReadMapEnd();
              }
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.Map) {
              {
                M_ShopPackageGoods = new Dictionary<int, sg.protocol.common.ShopPackageGoods>();
                TMap _map129 = iprot.ReadMapBegin();
                for( int _i130 = 0; _i130 < _map129.Count; ++_i130)
                {
                  int _key131;
                  sg.protocol.common.ShopPackageGoods _val132;
                  _key131 = iprot.ReadI32();
                  _val132 = new sg.protocol.common.ShopPackageGoods();
                  _val132.Read(iprot);
                  M_ShopPackageGoods[_key131] = _val132;
                }
                iprot.ReadMapEnd();
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
      TStruct struc = new TStruct("MsgAnsGetShopPackageProduct");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (Error != null && __isset.error) {
        field.Name = "error";
        field.Type = TType.Struct;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        Error.Write(oprot);
        oprot.WriteFieldEnd();
      }
      if (M_ShopPackage != null && __isset.m_ShopPackage) {
        field.Name = "m_ShopPackage";
        field.Type = TType.Map;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, M_ShopPackage.Count));
          foreach (int _iter133 in M_ShopPackage.Keys)
          {
            oprot.WriteI32(_iter133);
            M_ShopPackage[_iter133].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      if (M_ShopPackageGoods != null && __isset.m_ShopPackageGoods) {
        field.Name = "m_ShopPackageGoods";
        field.Type = TType.Map;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        {
          oprot.WriteMapBegin(new TMap(TType.I32, TType.Struct, M_ShopPackageGoods.Count));
          foreach (int _iter134 in M_ShopPackageGoods.Keys)
          {
            oprot.WriteI32(_iter134);
            M_ShopPackageGoods[_iter134].Write(oprot);
          }
          oprot.WriteMapEnd();
        }
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgAnsGetShopPackageProduct(");
      sb.Append("Error: ");
      sb.Append(Error== null ? "<null>" : Error.ToString());
      sb.Append(",M_ShopPackage: ");
      sb.Append(M_ShopPackage);
      sb.Append(",M_ShopPackageGoods: ");
      sb.Append(M_ShopPackageGoods);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
