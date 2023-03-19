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
  public partial class Notice : TBase
  {
    private int _i_NoticeSeq;
    private int _i_NoticeKind;
    private string _s_Title;
    private string _s_Content;
    private long _i_StartDate;
    private long _i_EndDate;

    public int I_NoticeSeq
    {
      get
      {
        return _i_NoticeSeq;
      }
      set
      {
        __isset.i_NoticeSeq = true;
        this._i_NoticeSeq = value;
      }
    }

    public int I_NoticeKind
    {
      get
      {
        return _i_NoticeKind;
      }
      set
      {
        __isset.i_NoticeKind = true;
        this._i_NoticeKind = value;
      }
    }

    public string S_Title
    {
      get
      {
        return _s_Title;
      }
      set
      {
        __isset.s_Title = true;
        this._s_Title = value;
      }
    }

    public string S_Content
    {
      get
      {
        return _s_Content;
      }
      set
      {
        __isset.s_Content = true;
        this._s_Content = value;
      }
    }

    public long I_StartDate
    {
      get
      {
        return _i_StartDate;
      }
      set
      {
        __isset.i_StartDate = true;
        this._i_StartDate = value;
      }
    }

    public long I_EndDate
    {
      get
      {
        return _i_EndDate;
      }
      set
      {
        __isset.i_EndDate = true;
        this._i_EndDate = value;
      }
    }


    public Isset __isset;
    #if !SILVERLIGHT
    [Serializable]
    #endif
    public struct Isset {
      public bool i_NoticeSeq;
      public bool i_NoticeKind;
      public bool s_Title;
      public bool s_Content;
      public bool i_StartDate;
      public bool i_EndDate;
    }

    public Notice() {
      this._i_NoticeSeq = 0;
      this.__isset.i_NoticeSeq = true;
      this._i_NoticeKind = 0;
      this.__isset.i_NoticeKind = true;
      this._s_Title = "";
      this.__isset.s_Title = true;
      this._s_Content = "";
      this.__isset.s_Content = true;
      this._i_StartDate = 0;
      this.__isset.i_StartDate = true;
      this._i_EndDate = 0;
      this.__isset.i_EndDate = true;
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
              I_NoticeSeq = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 2:
            if (field.Type == TType.I32) {
              I_NoticeKind = iprot.ReadI32();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 3:
            if (field.Type == TType.String) {
              S_Title = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 4:
            if (field.Type == TType.String) {
              S_Content = iprot.ReadString();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 5:
            if (field.Type == TType.I64) {
              I_StartDate = iprot.ReadI64();
            } else { 
              TProtocolUtil.Skip(iprot, field.Type);
            }
            break;
          case 6:
            if (field.Type == TType.I64) {
              I_EndDate = iprot.ReadI64();
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
      TStruct struc = new TStruct("Notice");
      oprot.WriteStructBegin(struc);
      TField field = new TField();
      if (__isset.i_NoticeSeq) {
        field.Name = "i_NoticeSeq";
        field.Type = TType.I32;
        field.ID = 1;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_NoticeSeq);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_NoticeKind) {
        field.Name = "i_NoticeKind";
        field.Type = TType.I32;
        field.ID = 2;
        oprot.WriteFieldBegin(field);
        oprot.WriteI32(I_NoticeKind);
        oprot.WriteFieldEnd();
      }
      if (S_Title != null && __isset.s_Title) {
        field.Name = "s_Title";
        field.Type = TType.String;
        field.ID = 3;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(S_Title);
        oprot.WriteFieldEnd();
      }
      if (S_Content != null && __isset.s_Content) {
        field.Name = "s_Content";
        field.Type = TType.String;
        field.ID = 4;
        oprot.WriteFieldBegin(field);
        oprot.WriteString(S_Content);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_StartDate) {
        field.Name = "i_StartDate";
        field.Type = TType.I64;
        field.ID = 5;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(I_StartDate);
        oprot.WriteFieldEnd();
      }
      if (__isset.i_EndDate) {
        field.Name = "i_EndDate";
        field.Type = TType.I64;
        field.ID = 6;
        oprot.WriteFieldBegin(field);
        oprot.WriteI64(I_EndDate);
        oprot.WriteFieldEnd();
      }
      oprot.WriteFieldStop();
      oprot.WriteStructEnd();
    }

    public override string ToString() {
      StringBuilder sb = new StringBuilder("Notice(");
      sb.Append("I_NoticeSeq: ");
      sb.Append(I_NoticeSeq);
      sb.Append(",I_NoticeKind: ");
      sb.Append(I_NoticeKind);
      sb.Append(",S_Title: ");
      sb.Append(S_Title);
      sb.Append(",S_Content: ");
      sb.Append(S_Content);
      sb.Append(",I_StartDate: ");
      sb.Append(I_StartDate);
      sb.Append(",I_EndDate: ");
      sb.Append(I_EndDate);
      sb.Append(")");
      return sb.ToString();
    }

  }

}
