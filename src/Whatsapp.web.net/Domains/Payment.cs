namespace Whatsapp.web.net.Domains;

public class Payment
{
    public object Id { get; private set; }

    public string PaymentCurrency { get; private set; }

    public double PaymentAmount1000 { get; private set; }

    public object PaymentMessageReceiverJid { get; private set; }

    public long PaymentTransactionTimestamp { get; private set; }

    /// <summary>
    ///    * Possible Status
    /// 0:UNKNOWN_STATUS
    /// 1:PROCESSING
    /// 2:SENT
    /// 3:NEED_TO_ACCEPT
    /// 4:COMPLETE
    /// 5:COULD_NOT_COMPLETE
    /// 6:REFUNDED
    /// 7:EXPIRED
    /// 8:REJECTED
    /// 9:CANCELLED
    /// 10:WAITING_FOR_PAYER
    /// 11:WAITING
    /// </summary>
    public int PaymentStatus { get; private set; }

    public int PaymentTxnStatus { get; private set; }

    public PaymentNoteMessage PaymentNoteMsg { get; private set; }

    public Payment(dynamic? data)
    {
        if (data is not null)
        {
            Patch(data);
        }
    }

    protected void Patch(dynamic? data)
    {
        if (data is null) return;
        Id = data.id;
        PaymentCurrency = data.paymentCurrency;
        PaymentAmount1000 = data.paymentAmount1000;
        PaymentMessageReceiverJid = data.paymentMessageReceiverJid;
        PaymentTransactionTimestamp = data.paymentTransactionTimestamp;
        PaymentStatus = data.paymentStatus;
        PaymentTxnStatus = data.paymentTxnStatus;
        PaymentNoteMsg = data.paymentNoteMsg;
    }
}