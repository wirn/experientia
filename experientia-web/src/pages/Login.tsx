import { useForm } from "react-hook-form";
import { z } from "zod";
import { zodResolver } from "@hookform/resolvers/zod";
import { useAuth } from "../auth/AuthContext";
import { useNavigate } from "react-router-dom";

const schema = z.object({
  email: z.string().email(),
  password: z.string().min(1, "Required"),
});
type FormData = z.infer<typeof schema>;

export default function Login() {
  const { login } = useAuth();
  const nav = useNavigate();
  const {
    register,
    handleSubmit,
    formState: { errors, isSubmitting },
    setError,
  } = useForm<FormData>({ resolver: zodResolver(schema) });

  async function onSubmit(data: FormData) {
    try {
      await login(data.email, data.password);
      nav("/dashboard"); // eller dit du vill
    } catch (e: unknown) {
      // Axios fel kan komma som string eller objekt; simpla fallback:
      const msg = (e as any)?.response?.data ?? "Login failed";
      setError("root", {
        message: typeof msg === "string" ? msg : "Login failed",
      });
    }
  }

  return (
    <form
      onSubmit={handleSubmit(onSubmit)}
      className="container"
      style={{ maxWidth: 420 }}
    >
      <h2 className="mt-4 mb-3">Login</h2>
      <div className="mb-3">
        <label className="form-label">Email</label>
        <input className="form-control" {...register("email")} />
        <div className="text-danger">{errors.email?.message}</div>
      </div>
      <div className="mb-3">
        <label className="form-label">Password</label>
        <input
          type="password"
          className="form-control"
          {...register("password")}
        />
        <div className="text-danger">{errors.password?.message}</div>
      </div>
      <button disabled={isSubmitting} className="btn btn-primary">
        Login
      </button>
      <div className="text-danger mt-2">{errors.root?.message}</div>
    </form>
  );
}
