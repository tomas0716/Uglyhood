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
  public partial class MsgAnsUserUnitActionLevelUp : TBase
  {
    private sg.protocol.basic.Error _error;
    private int _i_UnitId;
    private int _i_ActionId;
    private int _i_BeforeLevel;
    private int _i_AfterLevel;

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

    public int I_UnitId
    {
      get
      {
        return _i_UnitId;
      }
      set
      {
        __isset.i_UnitId = true;
        this._i_UnitId = value;
      }
    }

    public int I_ActionId
    {
      get
      {
        return _i_ActionId;
      }
      set
      {
        __isset.i_ActionId = true;
        this._i_ActionId = value;
      }
    }

    public int I_BeforeLevel
    {
      get
      {
        return _i_BeforeLevel;
      }
      set
      {
        __isset.i_BeforeLevel = true;
        this._i_BeforeLevel = value;
      }
    }

    public int I_AfterLevel
    {
      get
      {
        return _i_AfterLevel;
      }
      set
      {
        __isset.i_AfterLevel = true;
        this._i_AfterLevel = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool error;
      public bool i_UnitId;
      public bool i_ActionId;
      public bool i_BeforeLevel;
      public bool i_AfterLevel;
    }

    public MsgAnsUserUnitActionLevelUp() {
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
              I_UnitId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_ActionId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              I_BeforeLevel = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I32) {
              I_AfterLevel = iprot.ReadI32();
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
      TStruct struc = new TStruct("MsgAnsUserUnitActionLevelUp");
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
      if (__isset.i_UnitId) {
        field.Name = "i_UnitId";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_UnitId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_ActionId) {
        field.Name = "i_ActionId";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_ActionId);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_BeforeLevel) {
        field.Name = "i_BeforeLevel";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_BeforeLevel);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_AfterLevel) {
        field.Name = "i_AfterLevel";
        field.Type = TType.I32;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_AfterLevel);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("MsgAnsUserUnitActionLevelUp(");
      sb.Append("Error: ");
      sb.Append(Error== null ? "<null>" : Error.ToString());
      sb.Append(",I_UnitId: ");
      sb.Append(I_UnitId);
      sb.Append(",I_ActionId: ");
      sb.Append(I_ActionId);
      sb.Append(",I_BeforeLevel: ");
      sb.Append(I_BeforeLevel);
      sb.Append(",I_AfterLevel: ");
      sb.Append(I_AfterLevel);
      sb.Append(")");
      return sb.ToString();
    }

  }

}