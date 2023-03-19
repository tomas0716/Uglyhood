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

namespace sg.protocol.common
{

  #if !SILVERLIGHT
  [Serializable]
  #endif
  public partial class Summon : TBase
  {
    private int _i_Type;
    private int _i_Id;
    private int _i_Count;
    private int _i_BeforeRewardId;

    public int I_Type
    {
      get
      {
        return _i_Type;
      }
      set
      {
        __isset.i_Type = true;
        this._i_Type = value;
      }
    }

    public int I_Id
    {
      get
      {
        return _i_Id;
      }
      set
      {
        __isset.i_Id = true;
        this._i_Id = value;
      }
    }

    public int I_Count
    {
      get
      {
        return _i_Count;
      }
      set
      {
        __isset.i_Count = true;
        this._i_Count = value;
      }
    }

    public int I_BeforeRewardId
    {
      get
      {
        return _i_BeforeRewardId;
      }
      set
      {
        __isset.i_BeforeRewardId = true;
        this._i_BeforeRewardId = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_Type;
      public bool i_Id;
      public bool i_Count;
      public bool i_BeforeRewardId;
    }

    public Summon() {
      this._i_Type = 0;
      this.__isset.i_Type = true;
      this._i_Id = 0;
      this.__isset.i_Id = true;
      this._i_Count = 0;
      this.__isset.i_Count = true;
      this._i_BeforeRewardId = 0;
      this.__isset.i_BeforeRewardId = true;
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
              I_Type = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              I_Id = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.I32) {
              I_Count = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              I_BeforeRewardId = iprot.ReadI32();
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
      TStruct struc = new TStruct("Summon");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_Type) {
        field.Name = "i_Type";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Type);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_Id) {
        field.Name = "i_Id";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Id);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_Count) {
        field.Name = "i_Count";
        field.Type = TType.I32;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Count);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_BeforeRewardId) {
        field.Name = "i_BeforeRewardId";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_BeforeRewardId);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("Summon(");
      sb.Append("I_Type: ");
      sb.Append(I_Type);
      sb.Append(",I_Id: ");
      sb.Append(I_Id);
      sb.Append(",I_Count: ");
      sb.Append(I_Count);
      sb.Append(",I_BeforeRewardId: ");
      sb.Append(I_BeforeRewardId);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
