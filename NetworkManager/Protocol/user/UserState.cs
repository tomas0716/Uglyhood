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
  public partial class UserState : TBase
  {
    private int _i_Status;
    private bool _b_IsKick;
    private bool _b_IsBlock;
    private bool _b_IsTutorial;
    private bool _b_IsReview;

    public int I_Status
    {
      get
      {
        return _i_Status;
      }
      set
      {
        __isset.i_Status = true;
        this._i_Status = value;
      }
    }

    public bool B_IsKick
    {
      get
      {
        return _b_IsKick;
      }
      set
      {
        __isset.b_IsKick = true;
        this._b_IsKick = value;
      }
    }

    public bool B_IsBlock
    {
      get
      {
        return _b_IsBlock;
      }
      set
      {
        __isset.b_IsBlock = true;
        this._b_IsBlock = value;
      }
    }

    public bool B_IsTutorial
    {
      get
      {
        return _b_IsTutorial;
      }
      set
      {
        __isset.b_IsTutorial = true;
        this._b_IsTutorial = value;
      }
    }

    public bool B_IsReview
    {
      get
      {
        return _b_IsReview;
      }
      set
      {
        __isset.b_IsReview = true;
        this._b_IsReview = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_Status;
      public bool b_IsKick;
      public bool b_IsBlock;
      public bool b_IsTutorial;
      public bool b_IsReview;
    }

    public UserState() {
      this._i_Status = 0;
      this.__isset.i_Status = true;
      this._b_IsKick = false;
      this.__isset.b_IsKick = true;
      this._b_IsBlock = false;
      this.__isset.b_IsBlock = true;
      this._b_IsTutorial = false;
      this.__isset.b_IsTutorial = true;
      this._b_IsReview = false;
      this.__isset.b_IsReview = true;
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
              I_Status = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.Bool) {
              B_IsKick = iprot.ReadBool();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.Bool) {
              B_IsBlock = iprot.ReadBool();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.Bool) {
              B_IsTutorial = iprot.ReadBool();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.Bool) {
              B_IsReview = iprot.ReadBool();
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
      TStruct struc = new TStruct("UserState");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_Status) {
        field.Name = "i_Status";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_Status);
        oprot.WriteFieldEnd();
      }
      if (__isset.b_IsKick) {
        field.Name = "b_IsKick";
        field.Type = TType.Bool;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(B_IsKick);
        oprot.WriteFieldEnd();
      }
      if (__isset.b_IsBlock) {
        field.Name = "b_IsBlock";
        field.Type = TType.Bool;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(B_IsBlock);
        oprot.WriteFieldEnd();
      }
      if (__isset.b_IsTutorial) {
        field.Name = "b_IsTutorial";
        field.Type = TType.Bool;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(B_IsTutorial);
        oprot.WriteFieldEnd();
      }
      if (__isset.b_IsReview) {
        field.Name = "b_IsReview";
        field.Type = TType.Bool;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteBool(B_IsReview);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("UserState(");
      sb.Append("I_Status: ");
      sb.Append(I_Status);
      sb.Append(",B_IsKick: ");
      sb.Append(B_IsKick);
      sb.Append(",B_IsBlock: ");
      sb.Append(B_IsBlock);
      sb.Append(",B_IsTutorial: ");
      sb.Append(B_IsTutorial);
      sb.Append(",B_IsReview: ");
      sb.Append(B_IsReview);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
