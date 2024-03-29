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
  public partial class MsgAnsGetInGameDropItem : TBase
  {
    private sg.protocol.basic.Error _error;
    private int _i_ItemId;
    private int _i_ItemType;
    private int _i_ItemCount;

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

    public int I_ItemId
    {
      get
      {
        return _i_ItemId;
      }
      set
      {
        __isset.i_ItemId = true;
        this._i_ItemId = value;
      }
    }

    public int I_ItemType
    {
      get
      {
        return _i_ItemType;
      }
      set
      {
        __isset.i_ItemType = true;
        this._i_ItemType = value;
      }
    }

    public int I_ItemCount
    {
      get
      {
        return _i_ItemCount;
      }
      set
      {
        __isset.i_ItemCount = true;
        this._i_ItemCount = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool error;
      public bool i_ItemId;
      public bool i_ItemType;
      public bool i_ItemCount;
    }

    public MsgAnsGetInGameDropItem() {
      this._i_ItemId = 0;
      this.__isset.i_ItemId = true;
      this._i_ItemType = 0;
      this.__isset.i_ItemType = true;
      this._i_ItemCount = 0;
      this.__isset.i_ItemCount = true;
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
            if (field.Type == TType.I32) {
              I_ItemId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_ItemType = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              I_ItemCount = iprot.ReadI32();
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
      TStruct struc = new TStruct("MsgAnsGetInGameDropItem");
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
      if (__isset.i_ItemId) {
        field.Name = "i_ItemId";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ItemId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_ItemType) {
        field.Name = "i_ItemType";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ItemType);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_ItemCount) {
        field.Name = "i_ItemCount";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ItemCount);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgAnsGetInGameDropItem(");
      sb.Append("Error: ");
      sb.Append(Error== null ? "<null>" : Error.ToString());
      sb.Append(",I_ItemId: ");
      sb.Append(I_ItemId);
      sb.Append(",I_ItemType: ");
      sb.Append(I_ItemType);
      sb.Append(",I_ItemCount: ");
      sb.Append(I_ItemCount);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
