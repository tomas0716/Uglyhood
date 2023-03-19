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
  public partial class UserAttribute : TBase
  {
    private int _i_AttributeSeq;
    private int _i_AttributeId;
    private bool _b_IsNew;
    private int _i_PlayerSeq;

    public int I_AttributeSeq
    {
      get
      {
        return _i_AttributeSeq;
      }
      set
      {
        __isset.i_AttributeSeq = true;
        this._i_AttributeSeq = value;
      }
    }

    public int I_AttributeId
    {
      get
      {
        return _i_AttributeId;
      }
      set
      {
        __isset.i_AttributeId = true;
        this._i_AttributeId = value;
      }
    }

    public bool B_IsNew
    {
      get
      {
        return _b_IsNew;
      }
      set
      {
        __isset.b_IsNew = true;
        this._b_IsNew = value;
      }
    }

    public int I_PlayerSeq
    {
      get
      {
        return _i_PlayerSeq;
      }
      set
      {
        __isset.i_PlayerSeq = true;
        this._i_PlayerSeq = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_AttributeSeq;
      public bool i_AttributeId;
      public bool b_IsNew;
      public bool i_PlayerSeq;
    }

    public UserAttribute() {
      this._i_AttributeSeq = 0;
      this.__isset.i_AttributeSeq = true;
      this._i_AttributeId = 0;
      this.__isset.i_AttributeId = true;
      this._b_IsNew = false;
      this.__isset.b_IsNew = true;
      this._i_PlayerSeq = 0;
      this.__isset.i_PlayerSeq = true;
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
              I_AttributeSeq = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              I_AttributeId = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.Bool) {
              B_IsNew = iprot.ReadBool();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.I32) {
              I_PlayerSeq = iprot.ReadI32();
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
      TStruct struc = new TStruct("UserAttribute");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_AttributeSeq) {
        field.Name = "i_AttributeSeq";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_AttributeSeq);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_AttributeId) {
        field.Name = "i_AttributeId";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_AttributeId);
        oprot.WriteFieldEnd();
      }
      if (__isset.b_IsNew) {
        field.Name = "b_IsNew";
        field.Type = TType.Bool;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(B_IsNew);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_PlayerSeq) {
        field.Name = "i_PlayerSeq";
        field.Type = TType.I32;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_PlayerSeq);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("UserAttribute(");
      sb.Append("I_AttributeSeq: ");
      sb.Append(I_AttributeSeq);
      sb.Append(",I_AttributeId: ");
      sb.Append(I_AttributeId);
      sb.Append(",B_IsNew: ");
      sb.Append(B_IsNew);
      sb.Append(",I_PlayerSeq: ");
      sb.Append(I_PlayerSeq);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
