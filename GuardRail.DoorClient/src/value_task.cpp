#include "../include/value_task.h"

template <typename T>
value_task<T>::value_task()
{
	is_finished_ = true;
	thread_ = nullptr;
	throw std::exception();
}

template <typename T>
value_task<T>::value_task(std::thread* thread)
{
	is_finished_ = false;
	thread_ = thread;
}

template <typename T>
void value_task<T>::wait() const
{
	thread_->join();
}

template <typename T>
T value_task<T>::get_result()
{
	thread_->join();
	return result_;
}

template <typename T>
value_task<T> value_task<T>::run(T(*a)())
{
	std::thread thread(a);
	return value_task(thread);
}