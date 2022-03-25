using System.Text;

namespace Lucky {
    public class WrapperClientIdData : WrapperClientId, IData, ISignableData {
        public object Data { get; set; }

        public override StringBuilder GetSignData() {
            StringBuilder sb = base.GetSignData();
            if (Data == null) {
                sb.Append(nameof(Data));
            }
            else if (Data is ISignableData signData) {
                sb.Append(nameof(Data)).Append(signData.GetSignData().ToString());
            }
            else {
                sb.Append(nameof(Data)).Append(Data.ToString());
            }
            return sb;
        }
    }
}
